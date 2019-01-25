const path = require("path");

module.exports = {
  entry: path.join(__dirname, "./ClientTests.fsproj"),
  outDir: path.join(__dirname, "./jsOut/"),
  fable: { define: defineConstants() },
    babel: {
        plugins: ["@babel/transform-modules-commonjs"],
        // presets: [ ["env", {"modules": false}] ]
  },
  // allFiles: true
};

function defineConstants() {
  var ar = ["DEBUG"];

  return ar;
}
