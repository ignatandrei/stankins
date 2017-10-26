using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using StanskinsImplementation;
using StankinsInterfaces;

namespace HtmlGenerator.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet(int id)
        {
            
           return Page();
        }
        public string ExceptionMessage;
        [BindProperty]
        public string fileGenerated { get; set; }
        
        public async Task<IActionResult> OnPostAsync()
        {
           
            if (string.IsNullOrWhiteSpace(fileGenerated))
                return Page();
            try
            {
                IJob job = new SimpleJob();
                job.UnSerialize(fileGenerated);
                await job.Execute();
                return Page();
            }
            catch(Exception ex)
            {
                ExceptionMessage = ex.Message;
                return Page();
            }
        }
    }
}
