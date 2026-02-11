using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IClubUniquenessQueryHandler
{
    Task<bool> DoesClubCodeExist(string code);
    Task<bool> DoesClubNameExist(string name);
}
