using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace InventoryApp.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly MongoDbService _mongoDbService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
            _passwordHasher = new PasswordHasher<User>();
        }

        public AuthResponseDto Register(RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.IdNumber) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "All required fields must be filled."
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Password and confirmation password do not match."
                };
            }

            var existingUserByEmail = _mongoDbService.Users
                .Find(u => u.Email.ToLower() == request.Email.ToLower())
                .FirstOrDefault();

            if (existingUserByEmail != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "A user with this email already exists."
                };
            }

            var existingUserByIdNumber = _mongoDbService.Users
                .Find(u => u.IdNumber == request.IdNumber)
                .FirstOrDefault();

            if (existingUserByIdNumber != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "A user with this ID number already exists."
                };
            }

            Organization organization;
            string role;

            if (request.IsExistingOrganization)
            {
                if (request.ExistingOrganizationId == null || request.ExistingOrganizationId == Guid.Empty)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Please choose an existing organization."
                    };
                }

                organization = _mongoDbService.Organizations
                    .Find(o => o.OrganizationId == request.ExistingOrganizationId.Value && o.IsActive)
                    .FirstOrDefault();

                if (organization == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Selected organization was not found."
                    };
                }

                role = "Worker";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.OrganizationName) ||
                    string.IsNullOrWhiteSpace(request.OrganizationType))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Organization name and type are required for a new organization."
                    };
                }

                var existingOrganizationByName = _mongoDbService.Organizations
                    .Find(o => o.Name.ToLower() == request.OrganizationName.ToLower() && o.IsActive)
                    .FirstOrDefault();

                if (existingOrganizationByName != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "An organization with this name already exists. Please choose 'Existing organization'."
                    };
                }

                organization = new Organization
                {
                    Name = request.OrganizationName,
                    Type = request.OrganizationType
                };

                _mongoDbService.Organizations.InsertOne(organization);
                role = "Manager";
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IdNumber = request.IdNumber,
                Email = request.Email,
                Role = role,
                OrganizationId = organization.OrganizationId
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _mongoDbService.Users.InsertOne(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration completed successfully.",
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                OrganizationName = organization.Name,
                OrganizationId = user.OrganizationId,
                OrganizationType = organization.Type
            };
        }

        public AuthResponseDto Login(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email and password are required."
                };
            }

            var user = _mongoDbService.Users
                .Find(u =>
                    u.Email.ToLower() == request.Email.ToLower() &&
                    u.IsActive)
                .FirstOrDefault();

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            var organization = _mongoDbService.Organizations
                .Find(o => o.OrganizationId == user.OrganizationId)
                .FirstOrDefault();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                OrganizationName = organization?.Name,
                OrganizationId = user.OrganizationId,
                OrganizationType = organization?.Type
            };
        }
    }
}