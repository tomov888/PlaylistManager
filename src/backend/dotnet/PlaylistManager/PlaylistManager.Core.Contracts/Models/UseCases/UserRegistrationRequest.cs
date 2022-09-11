using PlaylistManager.Core.Domain.Enums;
using PlaylistManager.Core.Domain.Models;

namespace PlaylistManager.Core.Contracts.Models.UseCases;

public record UserRegistrationInfo(
	string Email, 
	string Username, 
	string Password, 
	DateTime DateOfBirth, 
	string PhotoUrl, 
	UserRole Role, 
	AuthProvider AuthProvider
);
