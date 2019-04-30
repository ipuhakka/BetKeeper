
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

export function betResultToRadioButtonValue(betResult){
  switch(betResult){
    case false:
      return 0;
    case true:
      return 1;
    default:
      return -1;
  }
}

/*
  Checks that value is valid double.
*/
export function isValidDouble(value){

  if (value.length === 0 ||
      value[0] === '.' ||
      value[value.length - 1] === '.')
  {
    return false;
  }

  if (Number.isNaN(value)){
    return false;
  }

  return true;
}

/*
  Checks a string only contains a letter, number, or character
  '-', '_', '.'. Returns true if is a valid string.
*/
export function isValidString(value){
  var nonAllowed = /[!@#$%^&*()+=[\]{};'':"\\|,<>/?]/;

  return !nonAllowed.test(value);
}
