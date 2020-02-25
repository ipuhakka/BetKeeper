import moment from 'moment';
import {isValidDouble, isValidString, deepCopy, shallowEquals, formatDateTime, camelCaseToText} from '../../src/js/utils';

var chai = require('chai');
var expect = chai.expect;

describe('camelCaseToText', function()
{
  it('Capitalizes first letter', function(done)
  {
    expect(camelCaseToText('test')).to.equal('Test');
    done();
  });

  it('Returns empty string on null and empty input', function(done)
  {
    expect(camelCaseToText('')).to.equal('');
    expect(camelCaseToText(null)).to.equal('');
    done();
  });

  it('Returns string splitted with spaces with first word capitalized', function(done)
  {
    const result = camelCaseToText('camelCasedText');
    expect(result.split(' ').length).to.equal(3);

    const splitted = result.split(' ');

    expect(splitted[0]).to.equal('Camel');
    expect(splitted[1]).to.equal('cased');
    expect(splitted[2]).to.equal('text');

    done();
  });
});

describe('formatDateTime', function()
{
  it('Formats datetime as utc to local time', function(done)
  {
    const utcMoment = moment();
    const localMoment = moment(formatDateTime(utcMoment)).local();

    expect(localMoment.isSame(utcMoment, 'seconds'));

    done();
  })
});

describe('shallowEquals', function(){

  it('returns true on equal objects', function(done){

    const testObj =
    {
        a: 1,
        b: "asd",
        c: true,
        d: null
    };

    const testObj2 =
    {
        a: 1,
        b: "asd",
        c: true,
        d: null
    }

    expect(shallowEquals(testObj, testObj2)).to.equal(true);
    done();
  });

  it('returns false on different number of keys', function(done){

    const testObj =
    {
        a: 1,
        b: "asd",
        c: true,
        d: null
    };

    const testObj2 =
    {
        a: 1,
        b: "asd",
        c: true
    }

    expect(shallowEquals(testObj, testObj2)).to.equal(false);
    done();
  });

  it('returns false on unequal objects', function(done){

    const testObj =
    {
        a: 1,
        b: "asd",
        c: true,
        d: null
    };

    const testObj2 =
    {
        a: 1,
        b: "asd",
        c: true,
        d: undefined
    }

    expect(shallowEquals(testObj, testObj2)).to.equal(false);
    done();
  });
});

describe('isValidString', function(){

  it ('returns true on not containing non-allowed characters', function(done){
    expect(isValidString('Tämän pitäisi olla validi')).to.equal(true);
    done();
  })

  it('returns false on containing non-allowed character', function(done){
    expect(isValidString('This ; should not pass')).to.equal(false);
    done();
  });
});

describe('isValidDouble', function(){

  it('returns false on empty value', function(done){
    expect(isValidDouble("")).to.equals(false);
    done();
  })

  it('returns false when value begins with decimal point', function(done){
    expect(isValidDouble(".2")).to.equals(false);
    done();
  })

  it('returns false when value ends with decimal point', function(done){
    expect(isValidDouble("2.")).to.equals(false);
    done();
  })

  it('returns true on valid double given as string', function(done){
    expect(isValidDouble("2.2")).to.equals(true);
    done();
  })

  it('returns true on valid double', function(done){
    expect(isValidDouble(2.2)).to.equals(true);
    done();
  })
});

describe('deepCopy', function(){

  it('objects are not equal after copy', function(done){

    const object1 = {
      var1: 1.0,
      var2: 1,
      var3: true,
      var4: 'test'
    };

    const object2 = object1;

    expect(object1).to.equal(object2);

    const deepCopiedObj = deepCopy(object2);

    expect(deepCopiedObj).to.not.equal(object2);

    done();
  });
})
