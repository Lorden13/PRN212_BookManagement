using System;
using System.Collections.Generic;

namespace BookManagement.SQLite;

public partial class SavedToken
{
    public string Id { get; set; } = null!;

    public string TokenValue { get; set; } = null!;
}
