﻿using AspNetCoreAPI.Data;
using AspNetCoreAPI.DTO;
using AspNetCoreAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("create-review")]
        public ActionResult<ReviewsDTO> CreateReview([FromBody] ReviewsDTO reviewsDTO)
        {
            try
            {
                using (var context = _context)
                {
                    var newReview = new ReviewsModel
                    {
                        ReviewId = reviewsDTO.ReviewId,
                        ReviewComment = reviewsDTO.ReviewComment,
                        ReviewCreator = reviewsDTO.ReviewCreator,
                        ReviewedProduct = reviewsDTO.ReviewedProduct,
                        StarRating = reviewsDTO.StarRating,
                        ReviewDate = reviewsDTO.ReviewDate,
                    };

                    context.Reviews.Add(newReview);
                    context.SaveChanges();

                    var info = new ReviewsDTO
                    {
                        ReviewId = reviewsDTO.ReviewId,
                        ReviewComment = reviewsDTO.ReviewComment,
                        ReviewCreator = reviewsDTO.ReviewCreator,
                        ReviewedProduct = reviewsDTO.ReviewedProduct,
                        StarRating = reviewsDTO.StarRating,
                        ReviewDate = reviewsDTO.ReviewDate,
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
        [Route("getReviews")]
        public ActionResult<IEnumerable<ReviewsDTO>> GetReviews(string productName)
        {
            List<ReviewsDTO> reviews = _context.Reviews
                .Where(r => r.ReviewedProduct == productName)
                .Select(r => new ReviewsDTO
                {
                    ReviewId = r.ReviewId,
                    ReviewComment = r.ReviewComment,
                    ReviewCreator = r.ReviewCreator,
                    ReviewedProduct = r.ReviewedProduct,
                    StarRating = r.StarRating,
                    ReviewDate = r.ReviewDate 
                })
                .ToList();

            return reviews;
        }
        [HttpDelete("{reviewId}")]
        public IActionResult DeleteReview(int reviewId)
        {
            try
            {
                using (var context = _context)
                {
                    var review = context.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);

                    if (review == null)
                    {
                        return NotFound();
                    }

                    context.Reviews.Remove(review);
                    context.SaveChanges();

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500);
            }
        }
    }
}
