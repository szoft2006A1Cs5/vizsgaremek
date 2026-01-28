using backend.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.UnitTests
{
    internal static class DbContextMock
    {
        public static DbContextOptions<Context> Create()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("comoveTest")
                .Options;

            return options;
        }
    }
}
