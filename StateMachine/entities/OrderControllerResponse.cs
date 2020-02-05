﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachine.entities
{
    public class OrderControllerResponse
    {
        public int Id { get; set; }

        public OrderDetails OrderDetails { get; set; }
        
        public OrderControllerResponseType ResponseType { get; set; }
        public string Message { get; set; }
    }

    public enum OrderControllerResponseType
    {
        Accept,
        Reject
    }
}