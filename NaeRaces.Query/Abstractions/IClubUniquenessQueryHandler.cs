using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IClubUniquenessQueryHandler
{
    Task<bool> DoesClubCodeExist(string code, Guid ignoreClubId);
    Task<bool> DoesClubNameExist(string name, Guid ignoreClubId);
}
