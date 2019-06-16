var chai = require('chai');
var expect = chai.expect;

import {isValidDouble, isValidString, deepCopy} from '../../../client/src/js/utils';

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
