using System;
using Coursework.Application.DTO;

namespace Coursework.Application.Common.Interface
{
	public interface IAuthenticate
	{
		Task<ResponseDTO> Register(CustomerRegisterRequestDTO model);
		//Task<IActionResult> Login(LoginRequestDTO mode);
	}
}

