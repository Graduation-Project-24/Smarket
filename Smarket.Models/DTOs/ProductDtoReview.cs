﻿using Microsoft.AspNetCore.Http;
using Smarket.Models.Dtos;

namespace Smarket.Models.DTOs
{
    public class ProductDtoReview
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubCategoryName { get; set; }
        public string ImageUrl { get; set; }
        public string BrandName { get; set; }
        public List<ReviewDto> Reviews { get; set; }

        public double AverageRate
        {
            get
            {
                if (Reviews == null || Reviews.Count == 0)
                    return 0;

                return Reviews.Average(r => r.Rate);
            }
        }
    }
}