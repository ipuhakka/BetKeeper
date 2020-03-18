var chai = require('chai');
var expect = chai.expect;
var _ = require('lodash');

import * as pageUtils from '../../src/js/pageUtils';

const tabPage = {
    key: 'page',
    components: [
        { 
            key: 'tab1',
            componentType: 'Tab',
            tabContent: [
                {
                    key: "name",
                },
                {
                    key: "name2",
                }
            ] 
        },
        { 
            key: 'tab2',
            componentType: 'Tab',
            tabContent: [
                {
                    key: "name3",
                },
                {
                    key: "name4",
                }
            ] 
        }
    ]
}

const componentPage = {
    key: 'page',
    components: [
        {
            key: "name",
        },
        {
            key: "name2",
        }
    ]
}

describe('findComponentFromPage', function()
{
    it('Finds component from tabs', function(done)
    {
        expect(pageUtils.findComponentFromPage(tabPage, "name2").key).to.equal('name2');
        done();
    });

    it('Finds component from regular page', function(done)
    {
        expect(pageUtils.findComponentFromPage(componentPage, "name2").key).to.equal('name2');
        done();
    });

    it('returns null when page not found', function(done)
    {
        expect(pageUtils.findComponentFromPage(componentPage, "unexisting key")).to.equal(null);
        expect(pageUtils.findComponentFromPage(tabPage, "unexisting key")).to.equal(null);
        done();
    })
});