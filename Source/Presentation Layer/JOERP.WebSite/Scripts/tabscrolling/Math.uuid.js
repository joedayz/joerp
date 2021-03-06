Math.uuid = (function() {
    var $ = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".split("");
    return function(_, G) {
        var C = $, F = [], D = Math.random;
        G = G || C.length;
        if (_) {
            for (var B = 0; B < _; B++) F[B] = C[0 | D() * G];
        } else {
            var A = 0, E;
            F[8] = F[13] = F[18] = F[23] = "-";
            F[14] = "4";
            for (B = 0; B < 36; B++)
                if (!F[B]) {
                    E = 0 | D() * 16;
                    F[B] = C[(B == 19) ? (E & 3) | 8 : E & 15];
                }
        }
        return F.join("");
    };
})();
var randomUUID = Math.uuid;