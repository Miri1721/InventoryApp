using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;

namespace InventoryApp.Api.Services
{
    public class AuthService : IAuthService
    {
        private static readonly List<Organization> _organizations = new();
        private static readonly List<User> _users = new();

        public AuthResponseDto Register(RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.IdNumber) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.OrganizationName) ||
                string.IsNullOrWhiteSpace(request.OrganizationType) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "All fields are required."
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

            if (_users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "A user with this email already exists."
                };
            }

            if (_users.Any(u => u.IdNumber == request.IdNumber))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "A user with this ID number already exists."
                };
            }

            var existingOrganization = _organizations.FirstOrDefault(o =>
                o.Name.Equals(request.OrganizationName, StringComparison.OrdinalIgnoreCase));

            Organization organization;
            string role;

            if (existingOrganization == null)
            {
                organization = new Organization
                {
                    Name = request.OrganizationName,
                    Type = request.OrganizationType
                };

                _organizations.Add(organization);
                role = "Manager";
            }
            else
            {
                organization = existingOrganization;
                role = "Worker";
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IdNumber = request.IdNumber,
                Email = request.Email,
                Password = request.Password,
                Role = role,
                OrganizationId = organization.OrganizationId
            };

            _users.Add(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration completed successfully.",
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                OrganizationName = organization.Name
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

            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == request.Password &&
                u.IsActive);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            var organization = _organizations.FirstOrDefault(o => o.OrganizationId == user.OrganizationId);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                OrganizationName = organization?.Name
            };
        }
    }
}