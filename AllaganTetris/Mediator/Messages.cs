namespace AllaganTetris.Mediator;

using System;
using DalaMock.Host.Mediator;

/// <summary>
/// Request that a window is toggled.
/// </summary>
/// <param name="WindowType">The type of the window.</param>
public record ToggleWindowMessage(Type WindowType) : MessageBase;

/// <summary>
/// Request that a window is closed.
/// </summary>
/// <param name="WindowType">The type of the window.</param>
public record OpenWindowMessage(Type WindowType) : MessageBase;

/// <summary>
/// Request that a window is closed.
/// </summary>
/// <param name="WindowType">The type of the window.</param>
public record CloseWindowMessage(Type WindowType) : MessageBase;

/// <summary>
/// Request that the tetris window be toggled.
/// </summary>
public record ToggleTetrisWindowMessage() : MessageBase;