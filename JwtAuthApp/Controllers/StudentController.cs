using BCrypt.Net;
using JwtAuthApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public ActionResult<Student> Register(StudentDTO studentDTO)
        {
            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(studentDTO.Password);

            // Save student details (in memory, consider a database for production)
            Student student = new Student
            {
                StudentName = studentDTO.StudentName,
                PasswordHash = passwordHash
            };

            // Return the created student
            return Ok(student);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(StudentDTO studentDTO)
        {
            // In a real-world scenario, you should validate the user from a database
            // For simplicity, assuming the student is already registered and stored in memory
            Student student = new Student
            {
                StudentName = "exampleUser", // Example user stored in memory
                PasswordHash = "$2a$11$FomZJ8X/Qi3kd4e3VOD7H.WGX5tcUhtCJpeX0O/rwqFvJPU2d3E8y" // Example hashed password
            };

            // Check if the provided username matches
            if (student.StudentName != studentDTO.StudentName)
            {
                return BadRequest("User Not Found");
            }

            // Verify the provided password against the hashed password
            if (!BCrypt.Net.BCrypt.Verify(studentDTO.Password, student.PasswordHash))
            {
                return BadRequest("Wrong Password");
            }

            // Create and return JWT token
            string token = CreateToken(student);
            return Ok(token);
        }

        private string CreateToken(Student student)
        {
            // Create claims for the JWT token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, student.StudentName)
            };

            // Get the secret key from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]));

            // Create signing credentials using the secret key and HMAC-SHA512 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Set token expiration time
            var expires = DateTime.Now.AddDays(1);

            // Create JWT token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            // Write and return the JWT token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}




















//using BCrypt.Net;
//using JwtAuthApp.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;

//namespace JwtAuthApp.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class StudentController : ControllerBase
//    {

//        public static Student student = new Student();
//        private readonly IConfiguration _configuration;

//        public StudentController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        [HttpPost("register")]
//        public ActionResult<Student> Register(StudentDTO studentDTO)
//        {
//            // hash the password :
//            string passwordHash = BCrypt.Net.BCrypt.HashPassword(studentDTO.Password);

//            student.StudentName = studentDTO.StudentName;
//            student.PasswordHash = passwordHash;

//            return Ok(student);
//        }

//        [HttpPost("login")]
//        public ActionResult<Student> Login(StudentDTO studentDTO)
//        {

//            if (student.StudentName != studentDTO.StudentName)
//            {
//                return BadRequest("User Not Found");
//            }

//            if (!BCrypt.Net.BCrypt.Verify(studentDTO.Password, student.PasswordHash))
//            {
//                return BadRequest("Wrong Password");
//            }
//            string token = CreateToken(student);

//            return Ok(token);
//        }

//        private string CreateToken(Student student)
//        {
//            List<Claim> claims = new List<Claim>
//            {
//                 new Claim(ClaimTypes.Name, student.StudentName)
//            };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(

//                _configuration.GetSection("AppSettings:Token").Value!
//             ));
//            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

//            var token = new JwtSecurityToken(
//                claims: claims,
//                expires: DateTime.Now.AddDays(1),
//                signingCredentials: cred
//                );

//            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

//            return jwt;
//        }





//    }
//}
