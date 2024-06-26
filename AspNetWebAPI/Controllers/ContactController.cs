﻿using AspNetCoreAPI.Data;
using AspNetCoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreAPI.DTO;

namespace AspNetCoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("create")]
        public ActionResult<ProblemsDTO> CreateProblem([FromBody] ProblemsDTO problemsDTO)
        {
            try
            {
                using (var context = _context)
                {
                    var newProblem = new ProblemsModel
                    {
                        NameSurname = problemsDTO.NameSurname,
                        Email = problemsDTO.Email,
                        Problem = problemsDTO.Problem,
                        ProblemDate = problemsDTO.ProblemDate,
                        ProblemStatus = "NotSolved"
                    };

                    context.Problems.Add(newProblem);
                    context.SaveChanges();

                    var info = new ProblemsDTO
                    {
                        NameSurname = problemsDTO.NameSurname,
                        Email = problemsDTO.Email,
                        Problem = problemsDTO.Problem,
                        ProblemDate = problemsDTO.ProblemDate,
                        ProblemStatus = "NotSolved"
                    };

                    return Ok(info);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
                return StatusCode(500, new { message = "Mame problem." });
            }
        }
        [HttpGet]
        public IEnumerable<ProblemsDTO> GetProblems()
        {
            IEnumerable<ProblemsModel> dbProblems = _context.Problems;

            return dbProblems.Select(dbProblems => new ProblemsDTO
            {
                NameSurname = dbProblems.NameSurname,
                Email = dbProblems.Email,
                Problem = dbProblems.Problem,
                ProblemDate = dbProblems.ProblemDate,
                ProblemId = dbProblems.ProblemId,
                ProblemStatus = dbProblems.ProblemStatus
            });
        }
        [HttpPut("changeProblemStatus/{problemId}")]
        public ActionResult<int> ChangeProblemStatus(int problemId)
        {
            var problem = _context.Problems.SingleOrDefault(p => p.ProblemId == problemId);

            if (problem == null)
            {
                return NotFound("Problem");
            }

            problem.ProblemStatus = "Solved";
            _context.SaveChanges();

            return Ok(problem.ProblemStatus);
        }
    }
}
