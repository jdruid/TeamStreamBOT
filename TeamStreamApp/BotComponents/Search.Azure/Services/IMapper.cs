using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamStreamApp.BotComponents.Search.Azure.Services
{
    public interface IMapper<T, TResult>
    {
        TResult Map(T item);
    }
}