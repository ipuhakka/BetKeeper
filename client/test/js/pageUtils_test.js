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
                    dataKey: 'dataKey1',
                    children: [
                        { componentKey: 'notMatch' },
                        { componentKey: 'match' }
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

const componentPage = {
    key: 'page',
    components: [
        {
            componentKey: "name",
            dataKey: 'dataKey1'
        },
        {
            componentKey: "name2",
            dataKey: 'dataKey2.dataKey2Nested2',
            children: [
                { componentKey: 'child-1'},
                { 
                    componentKey: 'child-2',
                    children: [
                        { componentKey: 'grandChild-1'},
                        { componentKey: 'grandChild-2'}
                    ]
                }
            ]
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

    it ('Finds child match', function(done)
    {
        expect(pageUtils.findComponentFromPage(tabPage, 'match').componentKey)
            .to.equal('match');
            done();
    });

    it('Finds grand child', function(done)
    {
        expect(pageUtils.findComponentFromPage(componentPage, 'grandChild-2').componentKey)
            .to.equal('grandChild-2');
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
    const replaceTabPage = {
        key: 'page',
        components: [
            { 
                componentKey: 'tab1',
                componentType: 'Tab',
                tabContent: [
                    {
                        componentKey: "name",
                        dataKey: 'dataKey1',
                        componentType: 'Container'
                    },
                    {
                        componentKey: "name2",
                        dataKey: 'dataKey2.dataKey2Nested2',
                        children: [
                            { 
                                componentKey: 'child1',
                                children: [
                                    { 
                                        componentKey: 'grandChild1',
                                        dataKey: 'replace this'
                                    }
                                ]
                            }
                        ]
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

    it('Replaces component on tab page', function(done)
    {
        const newContainer = { 
            componentKey: 'name', 
            componentType: 'Container', 
            children: [
            { componentKey: 'newChild' } 
        ]};

        const updatedPage = pageUtils.replaceComponent(replaceTabPage, newContainer);
        expect(updatedPage.components[0].tabContent[0].children[0].componentKey).to.equal('newChild');
        done();
    });

    it('Replaces component on normal page', function(done)
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

    it('Replaces component on a deeply nested page', function(done)
    {
        const componentPage = {
            key: 'page',
            components: [
                {
                    componentKey: "name",
                    dataKey: 'dataKey1',
                    children: [
                        { 
                            componentKey: 'child',
                            children: [
                                { 
                                    componentKey: 'grandChild',
                                    dataKey: 'grandChildDataKey1'
                                }
                            ]
                        }
                    ]
                },
                {
                    componentKey: "name2",
                    dataKey: 'dataKey2.dataKey2Nested2'
                }
            ]
        }

        const newComponent = { 
            componentKey: 'grandChild', 
            dataKey: 'grandChildDataKey2'
        };

        const updatedPage = pageUtils.replaceComponent(componentPage, newComponent);

        expect(updatedPage.components[0].children[0].children[0].dataKey).to.equal('grandChildDataKey2');
        done();
    });

    it('Replaces component on a deeply nested tab page', function(done)
    {
        const newComponent = { 
            componentKey: 'grandChild1', 
            dataKey: 'replaced datakey'
        };

        const updatedPage = pageUtils.replaceComponent(replaceTabPage, newComponent);

        expect(updatedPage.components[0].tabContent[1].children[0].children[0].dataKey).to.equal('replaced datakey');
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

describe('getActionData', function()
{
    it('Ignores undefined and null values', function(done)
    {
        const testData = {
            key1: null,
            key2: undefined
        };

        const components = [
            { componentKey: 'key1', storeDataAsArray: false},
            { componentKey: 'key2', storeDataAsArray: false},
            { componentKey: 'key3', storeDataAsArray: false}
        ];
        
        const parameters = pageUtils.getActionData(testData, ['key1', 'key2', 'key3'], components, []);

        // Not interested in components, drop
        delete parameters.components;

        expect(Object.keys(parameters).length).to.equal(0);
        done();
    })

    it('Works on single level data object', function(done)
    {
        const testData = {
            key1: 2,
            key2: 'string',
            key3: { someKey: 1, someOtherKey: 2 }
        }

        const components = [
            { componentKey: 'key2', storeDataAsArray: false },
            { componentKey: 'key3', storeDataAsArray: true },
        ]

        const parameters = pageUtils.getActionData(testData, ['key2', 'key3'], components, []);

        // Not interested in components, drop
        delete parameters.components;

        expect(Object.keys(parameters).length).to.equal(2);
        expect(parameters.key2).to.equal('string');
        expect(parameters.key3.length).to.equal(2);
        done();
    });

    it('Works on multi level data object', function(done)
    {
        const testData = {
            key1: 2,
            parentKey: {
                betTargets: {
                    target1: 1,
                    target2: 2
                }
            }
        };

        const components = [
            { componentKey: 'betTargets', storeDataAsArray: true}
        ];

        const parameters = pageUtils.getActionData(testData, ['betTargets'], components, []);

        // Not interested in components, drop
        delete parameters.components;

        expect(Object.keys(parameters).length).to.equal(1);
        expect(parameters.betTargets.length).to.equal(2);

        done();
    });
});

describe('replaceData', function()
{
    it('Replaces data on a single level object', function(done)
    {
        // Replace testKey3
        const testPage = {
            data: {
                testKey: 'nope',
                testKey2: 'nope',
                testKey3: 'yep'
            }
        };

        const result = pageUtils.replaceData(testPage, 'testKey3', 'found it');
        expect(result.data['testKey3']).to.equal('found it');
        done();
    });

    it('Replaces data on a multi level object', function(done)
    {
        // Replace innerInnerKey
        const testPage = {
            data: {
                testKey: 'nope',
                testKey2: {
                    innerKey1: 'test',
                    innerKey2: {
                        innerInnerKey: 'sgd'
                    }
                },
                testKey3: 'yep'
            }
        };

        const result = pageUtils.replaceData(testPage, 'innerInnerKey', 'found it');
        expect(result.data['testKey2']['innerKey2']['innerInnerKey']).to.equal('found it');
        done();
    });
})