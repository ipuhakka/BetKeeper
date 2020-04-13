import * as pageUtils from '../../src/js/pageUtils';

var chai = require('chai');
var expect = chai.expect;

const tabPage = {
    key: 'page',
    components: [
        { 
            componentKey: 'tab1',
            componentType: 'Tab',
            tabContent: [
                {
                    componentKey: "name",
                    dataKey: 'dataKey1'
                },
                {
                    componentKey: "name2",
                    dataKey: 'dataKey2.dataKey2Nested2'
                }
            ] 
        },
        { 
            componentKey: 'tab2',
            componentType: 'Tab',
            tabContent: [
                {
                    componentKey: "name3",
                    dataKey: 'dataKey3'
                },
                {
                    componentKey: "name4",
                }
            ] 
        }
    ]
}

const componentPage = {
    key: 'page',
    components: [
        {
            componentKey: "name",
            dataKey: 'dataKey1'
        },
        {
            componentKey: "name2",
            dataKey: 'dataKey2.dataKey2Nested2'
        }
    ]
}

const pageData = {
    dataKey1: 'dataValue',
    dataKey2: {
        dataKey2Nested1: 1,
        dataKey2Nested2: true
    },
    dataKey3: null,
    dataKey4: 'test'
};

describe('findComponentFromPage', function()
{
    it('Finds component from tabs', function(done)
    {
        expect(pageUtils.findComponentFromPage(tabPage, "name2").componentKey).to.equal('name2');
        done();
    });

    it('Finds component from regular page', function(done)
    {
        expect(pageUtils.findComponentFromPage(componentPage, "name2").componentKey).to.equal('name2');
        done();
    });

    it('Returns null when page not found', function(done)
    {
        expect(pageUtils.findComponentFromPage(componentPage, "unexisting key")).to.equal(null);
        expect(pageUtils.findComponentFromPage(tabPage, "unexisting key")).to.equal(null);
        done();
    })
});

describe('replaceComponent', function()
{
    const tabPage = {
        key: 'page',
        components: [
            { 
                componentKey: 'tab1',
                componentType: 'Tab',
                tabContent: [
                    {
                        componentKey: "name",
                        dataKey: 'dataKey1',
                        componentType: 'Container',
                        children: [
                            { componentKey: 'child1'}
                        ]
                    },
                    {
                        componentKey: "name2",
                        dataKey: 'dataKey2.dataKey2Nested2'
                    }
                ] 
            },
            { 
                componentKey: 'tab2',
                componentType: 'Tab',
                tabContent: [
                    {
                        componentKey: "name3",
                        dataKey: 'dataKey3'
                    },
                    {
                        componentKey: "name4",
                    }
                ] 
            }
        ]
    }

    it('replaces component on tab page', function(done)
    {
        const newContainer = { 
            componentKey: 'name', 
            componentType: 'Container', 
            children: [
            { componentKey: 'newChild' } 
        ]};

        const updatedPage = pageUtils.replaceComponent(tabPage, newContainer);

        expect(updatedPage.components[0].tabContent[0].children[0].componentKey).to.equal('newChild');
        done();
    });

    it('replaces component on normal page', function(done)
    {
        const componentPage = {
            key: 'page',
            components: [
                {
                    componentKey: "name",
                    dataKey: 'dataKey1'
                },
                {
                    componentKey: "name2",
                    dataKey: 'dataKey2.dataKey2Nested2'
                }
            ]
        }

        const newComponent = { 
            componentKey: 'name', 
            dataKey: 'dataKey2'
        };

        const updatedPage = pageUtils.replaceComponent(componentPage, newComponent);

        expect(updatedPage.components[0].dataKey).to.equal('dataKey2');
        done();
    });
});

describe('getDataFromComponents', function()
{
    it('Returns object with all data matching dataKeys in tab page', function(done)
    {
        const result = pageUtils.getDataFromComponents(tabPage.components, pageData);

        expect(Object.keys(result).length).to.equal(3);
        
        // object keys are component keys
        expect(result['name']).to.equal('dataValue');
        expect(result['name2']).to.equal(true);
        expect(result['name3']).to.equal(null);

        done();
    });

    it('Returns object with all data matching dataKeys in component page', function(done)
    {
        const result = pageUtils.getDataFromComponents(componentPage.components, pageData);

        expect(Object.keys(result).length).to.equal(2);
        
        // object keys are component keys
        expect(result['name']).to.equal('dataValue');
        expect(result['name2']).to.equal(true);

        done();
    });
});