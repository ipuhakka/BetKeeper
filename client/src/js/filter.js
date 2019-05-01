/*
  Filters a list. FilterOption is created
  per filter: When filtering number between values,
  options are created for both under and over values.

  filterOptions:
  [
    {
      key: filtered field key
      option: 'over', 'under', 'contains', 'is'. Contains only for string type, is for bool. Over and under
      exclude searched value.
      value: //filtered value.
    }
  ]
*/
export function filterList(betList, filterOptions){

  filterOptions.forEach(filterOption => {

    switch (filterOption.option){

      case 'over':
        betList = betList.filter(bet =>
          bet[filterOption.key] > filterOption.value);
        break;

      case 'under':
        betList = betList.filter(bet =>
          bet[filterOption.key] < filterOption.value);
        break;

      case 'is':
        betList = betList.filter(bet =>
          bet[filterOption.key] === filterOption.value);
        break;

      case 'contains':
        betList = betList.filter(bet =>
          bet[filterOption.key].includes(filterOption.value))
        break;

      default:
        break;
    }
  });

  return betList;
}

/*
Returns an array of filterOptions for single filter.

params:
  type: 'text', 'number', 'bool'.
  values: [].
  key: string. Referenes the filtered key on array.
  option: used by numeric filter to identify what type of filtering
    is done. 'Between', 'Over' or 'Under'.
*/
export function getFilterOptions(type, values, key, option){

  switch(type){
    case 'number':
      return getNumericFilters(values, key, option);
    case 'text':
      return [createFilter(values[0], key, 'contains')];

    case 'bool':
      return [createFilter(values[0], key, 'is')];

    default:
      break;
  }
}

/*
Returns numeric filters. If filter is used
to find where key is between numbers,
larger number is expected to be filtering values
lower than it, and smaller number filtering values
larger than it.
*/
function getNumericFilters(values, key, option){
  let filterOptions = [];

  if (option.toLowerCase() === 'between'){
    filterOptions.push(createFilter(Math.max(values), key, 'under'));
    filterOptions.push(createFilter(Math.min(values), key, 'over'));
  }

  else if (option.toLowerCase() === 'under'){
    filterOptions.push(createFilter(values[0], key, 'under'));
  }

  else if (option.toLowerCase() === 'over'){
    filterOptions.push(createFilter(values[0], key, 'over'));
  }

  return filterOptions;
}

function createFilter(values, key, option){
  return {
    key: key,
    option: option,
    value: values
  };
}
