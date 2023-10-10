﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly CheepRepository _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(CheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet(int pagen)
    {   
        if(pagen == null){pagen = 0;}
        Cheeps = _service.GetCheeps(pagen);
        return Page();
    }
}
