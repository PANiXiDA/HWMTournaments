﻿namespace DTOs.Models.Core;

public abstract class BaseDTO<TId>
{
    public TId Id { get; set; } = default!;
}
