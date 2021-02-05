import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import ListGroupRB from 'react-bootstrap/ListGroup';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import './ListGroup.css';
import * as Utils from '../../js/utils';
import * as PageUtils from '../../js/pageUtils';
import PageContent from './PageContent';
import * as PageActions from '../../actions/pageActions';
import { expandListGroupItem } from '../../js/Requests/Page';

class ListGroup extends Component
{
    constructor(props)
    {
        super(props);

        this.state = {
            selected: [],
            expandedContent: {}
        }

        this.handleGroupItemClick = this.handleGroupItemClick.bind(this);
        this.fetchItemContent = this.fetchItemContent.bind(this);
    }

    /**
     * Get data path for list group item
     * @param {*} dataItem 
     */
    getGroupItemDataPath(dataItem)
    {
        const { componentKey, keyField } = this.props;
        return `listGroup-${componentKey}-${dataItem[keyField]}`;
    }

    /**
     * Get expanded item content from server
     * @param {*} itemIdentifier 
     */
    fetchItemContent(itemIdentifier)
    {
        const { data, keyField } = this.props;

        expandListGroupItem({
            componentKey: this.props.componentKey,
            itemIdentifier: itemIdentifier
        }).then(response => 
        {
            const content = JSON.parse(response.responseText);
            const expandedContentState = { ...this.state.expandedContent };
            expandedContentState[itemIdentifier] = content;
            const dataItem = data.find(item => item[keyField] === itemIdentifier);

            this.setState({
                expandedContent: expandedContentState
            });

            PageActions.onDataChange(
                PageUtils.getActivePageName(),
                this.getGroupItemDataPath(dataItem),
                content.data);
        });
    }

    /**
     * Handle group item click
     * @param {*} itemIdentifier 
     */
    handleGroupItemClick(itemIdentifier)
    {
        const { selected } = this.state;
        const { mode, onSelect, dataPath, componentKey } = this.props;

        if (mode === 'DisplayOnly')
        {
            return;
        }

        /** Use selected state for both selectable and expandable type listgroup  */
        const newSelected = _.xor(selected, [itemIdentifier]);
        this.setState(
        {
            selected: newSelected
        });

        if (mode === 'Selectable')
        {
            /** Join dataPath and component key */
            const dataKey = _.compact([dataPath, componentKey]).join('.');
            /** Call select event only for selectable */
            onSelect(dataKey, newSelected);
        }

        if (mode === 'Expandable')
        {
            this.fetchItemContent(itemIdentifier);
        }
    }

    /**
     * Gets text shown for item.
     * @param {object} itemDefinition Object with fieldKey and fieldType string properties
     * @param {object} itemData
     */
    getItemText(itemDefinition, itemData)
    {
        let value;

        if (itemDefinition.fieldType === 'DateTime')
        {
            value = Utils.formatDateTime(itemData[itemDefinition.fieldKey])
        }
        else 
        {
            value = itemData[itemDefinition.fieldKey];
        }

        return _.compact([itemDefinition.fieldLegend, value]).join(': ');
    }

    /**
     * Returns page content for expanded item.
     * @param {*} dataItem 
     */
    getItemContent(dataItem)
    {
        const { keyField } = this.props;
        const { expandedContent } = this.state;

        // Get item content from expanded item content using data item key value
        const itemContent = expandedContent[dataItem[keyField]];
        if (!itemContent)
        {
            return null;
        }

        const { itemActions, itemFields } = expandedContent[dataItem[keyField]];
        return itemActions.map(actionButton =>
        {
            return {
                ...actionButton,
                actionDataKeys: actionButton.actionDataKeys.concat([this.getGroupItemDataPath(dataItem)])
            };
        }).concat(itemFields);
    }

    /**
     * Renders item content
     * @param {object} dataItem
     */
    renderItemContent(dataItem)
    {
        const { keyField } = this.props;

        const itemVisible = this.state.selected.includes(dataItem[keyField]);

        if (!itemVisible)
        {
            return null;
        }

        const itemContent = this.getItemContent(dataItem);
        return <div className={`listGroupItemBody${itemVisible ? ' visible' : ''}`}>
            { itemContent && <PageContent
                onFieldValueChange={(dataPath, newValue) => 
                {
                    PageActions.onDataChange(PageUtils.getActivePageName(), dataPath, newValue);
                }}
                components={itemContent}
                getButtonClick={this.props.onAction}
                className='listGroup-content'
                absoluteDataPath={this.getGroupItemDataPath(dataItem)}
                data={dataItem}/> 
            }
        </div>;
    }

    render()
    {
        const { data, headerItems, smallItems, keyField, mode } = this.props;

        const items = data.map((dataItem, i) =>
            {
                return <div key={`${keyField}-div-${i}`}>
                        <ListGroupItem 
                        action={mode !== 'DisplayMode'}
                        variant={this.state.selected.includes(dataItem[keyField]) ? 'info': null}
                        onClick={() => 
                        {
                            this.handleGroupItemClick(dataItem[keyField]);
                        }}
                        key={`${keyField}-header-${i}`}>
                        <div>{headerItems.map(item => this.getItemText(item, dataItem)).join(' ')}</div>
                        <div className='small-div'>{(smallItems || []).map(item => this.getItemText(item, dataItem)).join(' ')}</div>
                    </ListGroupItem>
                    {mode === 'Expandable' && this.renderItemContent(dataItem)}
                </div>;
            });

        return <ListGroupRB>{items}</ListGroupRB>;
    }
};

ListGroup.propTypes = {
    data: PropTypes.arrayOf(PropTypes.object).isRequired,
    keyField: PropTypes.string.isRequired,
    headerItems: PropTypes.arrayOf(
        PropTypes.shape({
            fieldKey: PropTypes.string.isRequired,
            fieldType: PropTypes.string.isRequired,
            fieldLegend: PropTypes.string
        })).isRequired,
    smallItems: PropTypes.arrayOf(
        PropTypes.shape({
            fieldKey: PropTypes.string.isRequired,
            fieldType: PropTypes.string.isRequired,
            fieldLegend: PropTypes.string
        })),
    mode: PropTypes.string.isRequired,
    componentKey: PropTypes.string,
    dataPath: PropTypes.string,
    onSelect: PropTypes.func.isRequired,
    onAction: PropTypes.func
}

export default ListGroup;