import * as autoFormatter from '../../src/js/autoformatter';

const chai = require('chai');
const expect = chai.expect;

describe('resultFormatter', () =>
{
    it('Adds dash after first character', done =>
    {
        const result = autoFormatter.format('Result', '4', null);

        expect(result).to.equal('4-');
        done();
    });

    it('Deos not modify value if a character was removed', done =>
    {
        const result = autoFormatter.format('Result', '4', '4-');

        expect(result).to.equal('4');
        done();
    });

    it('Deos not modify value if length is over 1', done =>
    {
        const result = autoFormatter.format('Result', '4-4', '4-');

        expect(result).to.equal('4-4');
        done();
    });
});