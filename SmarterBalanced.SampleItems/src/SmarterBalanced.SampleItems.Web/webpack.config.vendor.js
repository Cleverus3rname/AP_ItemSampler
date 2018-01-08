const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = (env) => {
    const extractCSS = new ExtractTextPlugin('vendor.css');
    const isDevBuild = !(env && env.prod);
    return [{
        stats: { modules: false },
        resolve: {
            extensions: ['.js', '.less', '.css']
        },
        module: {
            rules: [
                {
                    test: /\.less$/,
                    use: isDevBuild ?
                        ['style-loader', 'css-loader', 'less-loader'] :
                        ExtractTextPlugin.extract({ use: ['css-loader?minimize', 'less-loader'] })
                },
                {
                    test: /\.(woff|woff2|eot|ttf|svg)(\?|$)/,
                    use: 'url-loader?limit=100000'
                },
                {
                    test: /\.(png|jpg|gif)$/,
                    use: 'file-loader'
                },
                {
                    test: /\.css(\?|$)/,
                    use: extractCSS.extract([isDevBuild ? 'css-loader' : 'css-loader?minimize'])
                }
            ]
        },
        entry: {
            vendor: [
                'bootstrap',
                'bootstrap/less/bootstrap.less',
                'font-awesome/less/font-awesome.less',
                '@osu-cass/sb-components/lib/Assets/Styles/bundle.less',
                '@osu-cass/sb-components',
                'event-source-polyfill',
                'isomorphic-fetch',
                'react',
                'react-dom',
                'react-router-dom',
                'jquery',
                'typeface-pt-sans-caption/index.css',
                'typeface-pt-serif/index.css',
                'typeface-pt-serif-caption/index.css'
            ],
        },
        output: {
            path: path.join(__dirname, 'wwwroot', 'dist'),
            publicPath: 'dist/',
            filename: '[name].js',
            library: '[name]_[hash]',
        },
        plugins: [
            extractCSS,
            new webpack.ProvidePlugin({ $: 'jquery', jQuery: 'jquery' }), // Maps these identifiers to the jQuery package (because Bootstrap expects it to be a global variable)
            new webpack.DllPlugin({
                path: path.join(__dirname, 'wwwroot', 'dist', '[name]-manifest.json'),
                name: '[name]_[hash]'
            }),
            new webpack.DefinePlugin({
                'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
            }),
            new CopyWebpackPlugin([
                {
                    from: path.join(__dirname, 'node_modules', '@sbac/sbac-ui-kit/src/images'),
                    to: path.join(__dirname, 'wwwroot', 'Assets/Images')
                }
            ])
        ].concat(isDevBuild ? [] : [
            new webpack.optimize.UglifyJsPlugin()
        ])
    }];
};
