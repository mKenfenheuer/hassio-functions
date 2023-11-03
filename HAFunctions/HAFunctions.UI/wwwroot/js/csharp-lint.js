(function(mod) {
    if (typeof exports == "object" && typeof module == "object") // CommonJS
      mod(require("../../lib/codemirror"));
    else if (typeof define == "function" && define.amd) // AMD
      define(["../../lib/codemirror"], mod);
    else // Plain browser env
      mod(CodeMirror);
  })(function(CodeMirror) {
  "use strict";

  
  CodeMirror.registerHelper("lint", "clike", function(text) {
    return new Promise((resolve, reject) => 
    {
        $.ajax({
            type: "POST",
            url: './TryCompile',
            data: JSON.stringify({Code: text}),
            success: (data) => 
            {
                resolve(data.diagnostics);
            },
            error: (err) => 
            {
                reject(err);
            },
            dataType: "json",
            contentType: "application/json"
          });
    });
  });
  
});
  