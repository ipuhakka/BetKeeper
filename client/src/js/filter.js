import _ from 'lodash';
import moment from 'moment';

/**
 * Filters a list with selected filterOptions.
 * @param {array} list object array to filter
 * @param {object} filterOptions Filters used
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

      case 'valueList':
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

/**
 * Returns a filterOption for filtering a key.
 * @param {string} type
 * @param {string} key 
 * @param {array} values 
 */
export function getFilterOptions(type, key, values)
{
  switch(type){
    case 'number':
      const lowerLimit = values[0];
      const upperLimit = values[1];

      return createNumberFilter(key, lowerLimit, upperLimit);

    case 'string':
      return createFilter(type, key, values[0]);

    case 'valueList':
      return createFilter(type, key, values);

    case 'dateTime':     
      const after = values[0];
      const before = values[1];

      return createDateTimeFilter(key, after, before);

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
  const format = 'YYYY-MM-DD hh:mm:ss';
  const momentAfter = moment(after, format);
  const momentBefore = moment(before, format);

  return {
    type: 'dateTime',
    key: key,
    after: momentAfter.isValid()
      ? momentAfter
      : null,
    before: momentBefore.isValid()
      ? momentBefore
      : null
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
