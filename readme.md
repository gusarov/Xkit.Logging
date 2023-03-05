XkitConsoleLogger

Features:

1) Always uses dedicated background thread to write to the console. This helps with concurrency (decouple  from client thread) and performance (do not block client on console write)
2) AspNetCore Http Request & Response advanced logging
