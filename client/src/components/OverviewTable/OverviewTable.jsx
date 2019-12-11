import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import Table from 'react-bootstrap/Table';
import * as Sort from '../../js/sort.js';

class OverviewTable extends Component{
  constructor(props){
    super(props);

    this.state = {
        sortOrderHigh: false,
        sortKey: null,
        defaultSortOrder: []
    }
  }

  render()
  {
    const sortedOverviewItems = this.sortItems();

    const tableItems = [];

		for (var i = 0; i < sortedOverviewItems.length; i++){
			tableItems.push(<tr key={i}>
								<td>{sortedOverviewItems[i]["folder"]}</td>
								<td className={sortedOverviewItems[i]["moneyReturned"] >= 0 ? 'tableGreen' : 'tableRed'}>
                  {sortedOverviewItems[i]["moneyReturned"]}</td>
								<td className={sortedOverviewItems[i]["verifiedReturn"] >= 1 ? 'tableGreen' : 'tableRed'}>
                  {sortedOverviewItems[i]["verifiedReturn"]}</td>
								<td>{sortedOverviewItems[i]["winPercentage"]}</td>
							</tr>)
		}

  	return(<Table size="sm" hover>
  					<thead>
  						<tr>
  							<th className="clickable" onClick={this.changeSortParams.bind(this, "folder")}>{"Folder"}</th>
  							<th className="clickable" onClick={this.changeSortParams.bind(this, "moneyReturned")}>{"Money returned"}</th>
  							<th className="clickable" onClick={this.changeSortParams.bind(this, "verifiedReturn")}>{"Return percentage"}</th>
  							<th className="clickable" onClick={this.changeSortParams.bind(this, "winPercentage")}>{"Win percentage"}</th>
  						</tr>
  					</thead>
  					<tbody>{tableItems}</tbody>
				</Table>);
  }

  sortItems()
  {
    const { overviewItems } = this.props;
    const { sortKey, sortOrderHigh } = this.state;

    const sortBy = _.isNil(sortKey)
      ? Object.keys(overviewItems)[0]
      : sortKey;

    switch (sortBy)
    {
      case 'folder':
        return Sort.alphabetically(overviewItems, sortBy, sortOrderHigh);
      
      default:
        return Sort.byRank(overviewItems, sortBy, sortOrderHigh);
    }
  }

  /**
   * Changes sorting parameters. If key is not different from last key,
   * sort order is changed.
   * @param {string} key 
   */
  changeSortParams = (newSortKey) => {
    const { sortKey } = this.state;

    var sortOrderHigh = sortKey === newSortKey ? !this.state.sortOrderHigh : true;

    this.setState({
      sortKey: newSortKey,
      sortOrderHigh
    });
  }
};

const overviewItemProps = PropTypes.arrayOf(
  PropTypes.shape({
    folder: PropTypes.string,
    moneyReturned: PropTypes.number,
    verifiedReturn: PropTypes.number,
    winPercentage: PropTypes.number
  }));

OverviewTable.propTypes = {

  overviewItems: overviewItemProps.isRequired,

  defaultSort: overviewItemProps
};

export default OverviewTable;
