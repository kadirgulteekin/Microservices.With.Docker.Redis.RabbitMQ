﻿namespace Web.Models
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? City { get; set; }


        public IEnumerable<string> GetUserProps()
        {
            yield return Id!;
            yield return Email!;
            yield return UserName!;
            yield return City!;
        }
    }
}
