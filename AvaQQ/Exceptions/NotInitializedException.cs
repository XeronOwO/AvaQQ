using AvaQQ.Resources;

namespace AvaQQ.Exceptions;

internal class NotInitializedException(string name) : Exception(string.Format(SR.ExceptionNotInitialized, name));
