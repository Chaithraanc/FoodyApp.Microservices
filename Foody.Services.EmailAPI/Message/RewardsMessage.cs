﻿namespace Foody.Services.EmailAPI.Message
{
    public class RewardsMessage
    {
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public int RewardsActivity { get; set; }
    }
}
