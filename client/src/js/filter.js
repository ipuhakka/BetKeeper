import _ from 'lodash';
import moment from 'moment';

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
        if (filterOption.value.length === 0)
        {
          break;
        }

        list = list.filter(item =>
          _.some(filterOption.value, value =>
            value === item[filterOption.key]));
        break;

      case 'string':
        list = list.filter(item =>
          item[filterOption.key]
            .toLowerCase()
            .includes(filterOption.value.toLowerCase()))
        break;

      case 'dateTime':
        if (!_.isNil(filterOption.before))
        {
          list = list.filter(item =>
            moment(item[filterOption.key])
              .isSameOrBefore(filterOption.before))
        }

        if (!_.isNil(filterOption.after))
        {
          list = list.filter(item =>
            moment(item[filterOption.key])
              .isSameOrAfter(filterOption.after))
        }


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
  values: Array of filtered values. With number and dateTime filter,
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
      return createFilter(type, key, values);

    case 'dateTime':
      return createDateTimeFilter(key, values[0], values[1]);

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

function createDateTimeFilter(key, after, before)
{
  return {
    type: 'dateTime',
    after: moment(after),
    before: moment(before),
    ...key
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
