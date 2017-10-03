var path = require('path');

module.exports = {
  entry: {
      "sb.itemsampler.aboutitems": "./Scripts/AboutItem/Index.tsx",
      "sb.itemsampler.itemssearch": "./Scripts/ItemSearch/Index.tsx",
      "sb.itemsampler.itempagecontainer": "./Scripts/ItemPage/Index.tsx",
      "polyfill": "./Scripts/Polyfill.ts"
  },

  output: {
    path: path.join(__dirname, 'wwwroot/scripts/'),
    filename: "[name].js",
    libraryTarget: "var",
    library: "EntryPoint"
  },

  // Enable sourcemaps for debugging webpack's output.
  devtool: "source-map",

  resolve: {
    // Add '.ts' and '.tsx' as resolvable extensions.
    extensions: [".ts", ".tsx", ".js", ".json"]
  },

  module: {
    rules: [
        // All files with a '.ts' or '.tsx' extension will be handled by 'awesome-typescript-loader'.
        { test: /\.tsx?$/, loader: "awesome-typescript-loader", options: { configFileName: path.join(__dirname, 'Scripts/tsconfig.json') } },

        // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
        { enforce: "pre", test: /\.js$/, loader: "source-map-loader" }
    ]
  },

  externals: {
    "react": "React",
    "react-dom": "ReactDOM",
    "jquery": "jQuery"
  }
};