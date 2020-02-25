import React, { Component } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import _ from 'lodash';
import RBTable from 'react-bootstrap/Table';
import * as utils from '../../js/utils';

class Table extends Component
{
    getHeaders()
    {
        const { data, dataKey } = this.props;

        // Get keys from first row
        const keys = Object.keys(data[dataKey][0]);

        return <tr>
            {_.map(keys, key => {
                return <th>{utils.camelCaseToText(key)}</th>;
            })}
        </tr>
    }

    render()
    {
        const { dataKey, data } = this.props;

        if (_.isNil(data[dataKey]))
        {
            throw new Error(`Specified data key ${dataKey} not found`);
        }

        return <RBTable>
            <thead>
                {this.getHeaders()}
            </thead>
        </RBTable>
    }
};

Table.propTypes = {
    dataKey: PropTypes.string.isRequired,
    data: PropTypes.object
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