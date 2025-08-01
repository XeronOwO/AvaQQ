using Avalonia.Media.Imaging;

namespace AvaQQ.SDK.Entities;

public record struct AvatarChangedInfo(Bitmap? Old, Bitmap New);
