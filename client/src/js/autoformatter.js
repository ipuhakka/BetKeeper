/**
 * Format inputs
 * @param {*} formatter 
 * @param {*} value
 * @param {*} oldValue
 */
export function format(formatter, value, oldValue)
{
    switch(formatter)
    {
        case 'Result':
            return formatResult(value, oldValue);

        default:
            throw new Error(`Formatter ${formatter} not implemented`);
    }
}

/**
 * Format result inputs. Add dash automatically after first character
 * @param {*} value 
 * @param {*} oldValue 
 */
function formatResult(value, oldValue)
{
    if (oldValue && value.length < oldValue.length)
    {
        // If a character was removed, don't format
        return value;
    }

    if (value.length === 1)
    {
        return `${value}-`;
    }

    return value;
}