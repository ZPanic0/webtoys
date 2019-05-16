/// <binding ProjectOpened='Watch - Development' />
const path = require('path');

module.exports = [
    {
        entry: {
            client: './Components/site.js'
        },
        output: {
            path: path.resolve(__dirname, 'wwwroot/js'),
            filename: 'site.js'
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        externals: {
            react: 'React',
            'react-dom': 'ReactDOM'
        },
        mode: 'development'
    },
    {
        entry: {
            client: './Components/site.js'
        },
        output: {
            path: path.resolve(__dirname, 'wwwroot/js'),
            filename: 'site.min.js'
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        externals: {
            react: 'React',
            'react-dom': 'ReactDOM'
        },
        mode: 'production'
    }
];