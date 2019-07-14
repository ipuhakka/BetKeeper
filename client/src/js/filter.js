import _ from 'lodash';

/*
  Filters a list. FilterOption is created
  per filter: When filtering number between values,
  options are created for both under and over values.

  filterOptions-model for boolean, dateTime and string type:
  {
    key: filtered field key
    value: //filtered value.
    type: //'string', 'boolean', 'dateTime'
  }

  model for number-filter:
  {
    key: filtered field key
    lowerLimit: //values under this are excluded
    upperLimit: //values over this are excluded
    type: //'number',
  }
*/
export function filterList(list, filterOptions){

  filterOptions.forEach(filterOption => {

    switch (filterOption.type){

      case 'number':
        if (!_.isNil(filterOption.lowerLimit))
        {
          list = list.filter(item =>
            item[filterOption.key] >= filterOption.lowerLimit);
        }
        if (!_.isNil(filterOption.upperLimit))
        {
          list = list.filter(item =>
            item[filterOption.key] <= filterOption.upperLimit);
        }
        break;

      case 'boolean':
        list = list.filter(item =>
          item[filterOption.key] === filterOption.value);
        break;

      case 'string':
        list = list.filter(item =>
          item[filterOption.key].includes(filterOption.value))
        break;

      case 'dateTime': //not implemented
        break;

      default:
        break;
    }
  });

  return list;
}

/*
Returns a filterOption for filtering a key.

params:
  type: 'string', 'number', 'boolean', 'dateTime'.
  key: string. References the filtered key on array.
  values: Array of fitlered values.with number filter,
    first value is considered the lower limit and second as
    upper limit.
*/
export function getFilterOptions(type, key, values)
{
  switch(type){
    case 'number':
      return createNumberFilter(key, values[0], values[1]);

    case 'string':
      return createFilter(type, key, values[0]);

    case 'boolean':
      return createFilter(type, key, values[0]);

    case 'dateTime':
      // Not implemented
      return;

    default:
      return;
  }
}

function createFilter(type, key, value)
{
  return {
    key: key,
    value: value,
    type: type
  };
}

function createNumberFilter(key, lowerLimit, upperLimit)
{
  return {
    key: key,
    lowerLimit: lowerLimit,
    upperLimit: upperLimit,
    type: 'number'
  };
}
