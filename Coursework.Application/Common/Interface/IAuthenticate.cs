﻿using System;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace Coursework.Application.Common.Interface
{
	public interface IAuthenticate
	{
		Task<ResponseDTO> Register(CustomerRegisterRequestDTO model);
		//Task<IActionResult> Login(LoginRequestDTO mode);
	}
}
