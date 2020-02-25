import React, { Component } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import _ from 'lodash';
import RBTable from 'react-bootstrap/Table';
import * as utils from '../../js/utils';

class Table extends Component
{
    getVisibleColumns()
    {
        const { data, dataKey, hiddenKeys } = this.props;

        return _.filter(Object.keys(data[dataKey][0]), key =>
        {
            return !_.includes(hiddenKeys, key);
        });
    }

    getHeaders()
    {
        const keys = this.getVisibleColumns();

        return <tr>
            {_.map(keys, key => {
                return <th key={`table-header-${key}`}>{utils.camelCaseToText(key)}</th>;
            })}
        </tr>
    }

    getRows()
    {
        const { data, dataKey } = this.props;

        return _.map(data[dataKey], (dataRow, i) =>
        {
            const keys = this.getVisibleColumns();

            const cells = _.map(keys, (key, i) =>
            {
                return <td key={`data-row-td-${i}-key`}>{dataRow[key]}</td>;
            })

            return <tr key={`data-row-tr-${i}-key`}>{cells}</tr>;
        });
    }

    render()
    {
        const { dataKey, data } = this.props;

        if (_.isNil(data[dataKey]))
        {
            throw new Error(`Specified data key ${dataKey} not found`);
        }

        // TODO: Table body
        return <RBTable hover responsive striped>
            <thead>
                {this.getHeaders()}
            </thead>
            <tbody>
                {this.getRows()}
            </tbody>
        </RBTable>
    }
};

Table.propTypes = {
    dataKey: PropTypes.string.isRequired,
    data: PropTypes.object,
    hiddenKeys: PropTypes.arrayOf(PropTypes.string)
};

const mapStateToProps = (state) => 
{
    const pathname = window.location.pathname;
    // Parse page name from path
    const pageKey = pathname.substring(pathname.lastIndexOf('/') + 1);

    return {
        data: _.find(state.pages.pages, page =>
            page.key === pageKey).data || {} 
    };
};

export default connect(mapStateToProps)(Table);