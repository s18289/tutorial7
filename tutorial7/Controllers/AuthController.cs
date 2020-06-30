﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components;
using System;
using System.Data.SqlClient;
using tutorial7.DTO_s.Requests;
using tutorial7.Models;
using tutorial7.Generators;

namespace tutorial7.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly string ConnectionString = "Data Source=db-mssql;Initial Catalog=s18289;Integrated Security=True";

        public AuthController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        [HttpGet]
        public IActionResult Login(LoginRequest request)
        {
            var student = new Student();

            using var con = new SqlConnection(ConnectionString);
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                var salt = String.Empty;
                var password = String.Empty;
                com.CommandText = "Select IndexNumber, FirstName Password, Salt From Student Where IndexNumber = @login";
                com.Parameters.AddWithValue("login", request.Login);
                con.Open();
                var dr = com.ExecuteReader();

                if (!dr.Read()) return NotFound("Student was not found.");

                student.IndexNumber = dr["IndexNumber"].ToString();
                student.FirstName = dr["FirstName"].ToString();
                password = dr["Password"].ToString();
                salt = dr["Salt"].ToString();

                var passToCompare = HashPasswordGenerator.HashPasswordGen(request.Password, salt);

                if (!password.Equals(passToCompare)) return BadRequest("Wrong login or password");

                dr.Close();

                var userclaim = new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, student.IndexNumber),
                        new Claim(ClaimTypes.Name, student.FirstName),
                        new Claim(ClaimTypes.Role, "Student"),
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "Gakko",
                    audience: "Students",
                    claims: userclaim,
                    expires: DateTime.Now.AddMinutes(1),
                    signingCredentials: creds
                );

                student.RefreshToken = Guid.NewGuid().ToString();
                student.RefreshTokenExpirationDate = DateTime.Now.AddDays(1);

                com.CommandText = "Update Student set RefreshToken = @RefreshToken and RefreshTokenExpirationDate = @ExpDate";
                com.Parameters.AddWithValue("RefreshToken", student.RefreshToken);
                com.Parameters.AddWithValue("ExpDate", student.RefreshTokenExpirationDate);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = student.RefreshToken
                });

            }
        }

        [HttpPost("{refreshToken}/refresh")]
        public IActionResult RefreshToken([FromRoute]string refreshToken)
        {
            DateTime expirationDate = DateTime.Now;
            var student = new Student();
            using (var con = new SqlConnection(ConnectionString))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "Select * from Student Where RefreshToken = @RefreshToken";
                    com.Parameters.AddWithValue("RefreshToken", refreshToken);
                    con.Open();

                    var dr = com.ExecuteReader();
                    if (!dr.Read()) return NotFound("Cannot find such token");

                    expirationDate = Convert.ToDateTime(dr["RefreshTokenExpirationDate"].ToString());
                    student.IndexNumber = dr["IndexNumber"].ToString();
                    student.FirstName = dr["FirstName"].ToString();


                    if (expirationDate < DateTime.Now) return BadRequest("Refrehs token has expired");

                    var userclaim = new[] {
                            new Claim(ClaimTypes.NameIdentifier, student.IndexNumber),
                            new Claim(ClaimTypes.Name, student.FirstName),
                            new Claim(ClaimTypes.Role, "Student"),
                        };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "Gakko",
                        audience: "Students",
                        claims: userclaim,
                        expires: DateTime.Now.AddMinutes(1),
                        signingCredentials: creds
                    );

                    student.RefreshToken = Guid.NewGuid().ToString();
                    student.RefreshTokenExpirationDate = DateTime.Now.AddDays(1);

                    dr.Close();
                    com.CommandText = "Update Student set RefreshToken = @RefreshToken and RefreshTokenExpirationDate = @ExpDate";
                    com.Parameters.AddWithValue("RefreshToken", student.RefreshToken);
                    com.Parameters.AddWithValue("ExpDate", student.RefreshTokenExpirationDate);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        refreshToken = student.RefreshToken
                    });
                }
            }
        }
    }
}