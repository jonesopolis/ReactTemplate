var path = require('path');
const ExtractTextPlugin = require("extract-text-webpack-plugin");

module.exports = {
    entry: {
        login: './src/login.jsx',
        app: './src/app.jsx'
    },
    output: { path: __dirname, filename: './wwwroot/[name].js' },
    module: {
        rules: [
            {
                test: /\.jsx$/,
                loader: 'babel-loader',
                exclude: /node_modules/,
                query: {
                    presets: ['es2015', 'react']
                }
            },
            {
                test: /\.css/,
                loader: 'css-loader'
            },
            {
                test: /\.less/,
                use: ExtractTextPlugin.extract({
                    use: [{
                        loader: "css-loader"
                    }, {
                        loader: "less-loader"
                    }]
                })
            },
            {
                test: /\.(png|jpg|gif|svg|ttf|woff2|woff|eot)$/,
                loader: 'file-loader',
                options: {
                    name: './wwwroot/[name].[ext]?[hash]'
                }
            }
        ]
    },
    resolve: {
        modules: [
            path.resolve('./src'),
            path.resolve('./node_modules'),
            path.resolve('./less')
        ]
    },
    plugins: [
        new ExtractTextPlugin("./wwwroot/[name].css")
    ]
};