﻿namespace Application.DTOs.WordPress
{
    public class CategoryDTO : BasedWordPressDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
    }
}
