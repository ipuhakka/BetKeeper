import ConstVars from '../js/consts';

/**
 * Sends client error information to server
 * @param {string} message 
 * @param {string} stacktrace 
 * @param {string} url 
 * @param {number} columnNumber 
 * @param {number} lineNumber 
 */
export function logError(message, stacktrace, url, columnNumber, lineNumber)
{
    fetch(`${ConstVars.URI}error`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },
        body: JSON.stringify({
            stacktrace: stacktrace,
            message: message,
            url: url,
            columnNumber: columnNumber,
            lineNumber: lineNumber
        })
    });
}