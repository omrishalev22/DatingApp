using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DatingApp.API.Helpers {
    public static class Extensions {

            public static void AddApplicationErrors(this HttpResponse response, string message) {
                response.Headers.Add("Application-Error",message);
                response.Headers.Add("Access-Control-Expose-Headers","Application-Error"); 
                response.Headers.Add("Access-Control-Allow-Origin","*"); 
            }

            public static void AddPagination ( this HttpResponse response, PaginationHeader pagination)
            {
                response.Headers.Add("Pagination",JsonConvert.SerializeObject(pagination));
                response.Headers.Add("Access-Control-Expose-Headers","Pagination"); 
            }

            public static int CalculateAge(this DateTime time){
                var age = DateTime.Now.Year - time.Year;
                // in case it wasn't its birthdate yet
                if(time.AddYears(age) < DateTime.Today)
                    age--;
                return age;
            }
    }
}