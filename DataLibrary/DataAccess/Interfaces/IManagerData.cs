﻿using DataLibrary.Models;
using DataLibrary.Models.Database;

namespace DataLibrary.DataAccess.Interfaces;

public interface IManagerData
{
    Task<IEnumerable<Manager>> GetSinceAsync(DateTime fromDate, string connStrKey);
}
