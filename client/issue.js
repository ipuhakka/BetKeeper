/**
 * Check actual point of error from minified code. To do this, enable source-mapping from webpack.config.prod.js
 *   optimization: {
 *       minimizer: [new UglifyJsPlugin({
 *       sourceMap: true
 *   })]
 * 
 *  Build app
 *  npm run build
 * 
 *  Install package source-map
 *  npm install -g source-map
 * 
 *  Run this script
 *  node issue.js {sourceMapPath} {line} {column}
 * 
 */

const [sourceMapPath, line, column] = process.argv.slice(2);
const fs = require('fs');

const sourceMap = require('source-map');

const sourceMapConsumer = new sourceMap.SourceMapConsumer(fs.readFileSync(sourceMapPath, "utf8"));
console.log(sourceMapConsumer.originalPositionFor({line: parseInt(line), column: parseInt(column)}));