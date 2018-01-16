const path = require("path");
const webpack = require("webpack");
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const bundleOutputDir = "./wwwroot/dist";

module.exports = env => {
  const isDevBuild = !(env && env.prod);
  return [
    {
      stats: { modules: false },
      entry: { main: "./Client/boot.tsx" },
      resolve: { extensions: [".js", ".jsx", ".ts", ".tsx"] },
      output: {
        path: path.join(__dirname, bundleOutputDir),
        filename: "[name].js",
        publicPath: "dist/"
      },
      resolve: {
        extensions: [".ts", ".tsx", ".js", ".jsx", ".json"],
        modules: [
          path.join(__dirname, "node_modules"),
          path.join(__dirname, "./client")
        ]
      },
      module: {
        rules: [
          {
            test: /\.js$/,
            use: ["source-map-loader"],
            enforce: "pre"
          },
          {
            test: /\.tsx?$/,
            include: /Client/,
            use: "ts-loader"
          },
          {
            test: /\.css$/,
            use: isDevBuild
              ? ["style-loader", "css-loader"]
              : ExtractTextPlugin.extract({ use: "css-loader?minimize" })
          },
          {
            test: /\.less$/,
            use: isDevBuild
              ? ["style-loader", "css-loader", "less-loader"]
              : ExtractTextPlugin.extract({
                  use: ["css-loader?minimize", "less-loader"]
                })
          },
          {
            test: /\.(png|svg|jpg|gif)$/,
            use: "file-loader"
          },
          {
            test: /\.(woff|woff2|eot|ttf)(\?|$)/,
            use: "url-loader?limit=100000"
          }
        ]
      },
      plugins: [
        new webpack.DllReferencePlugin({
          context: __dirname,
          manifest: require("./wwwroot/dist/vendor-manifest.json")
        })
      ].concat(
        isDevBuild
          ? [
              // Plugins that apply in development builds only
              new webpack.SourceMapDevToolPlugin({
                moduleFilenameTemplate: path.relative(
                  bundleOutputDir,
                  "[resourcePath]"
                ) // Point sourcemap entries to the original file locations on disk
              })
            ]
          : [
              // Plugins that apply in production builds only
              new webpack.optimize.UglifyJsPlugin(),
              new ExtractTextPlugin("site.css")
            ]
      )
    }
  ];
};
