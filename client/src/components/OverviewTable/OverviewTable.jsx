import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Table from 'react-bootstrap/Table';
import * as Sort from '../../js/sort.js';
import {deepCopy} from '../../js/utils.js';

class OverviewTable extends Component{
  constructor(props){
    super(props);

    this.state = {
        sortedOverviewItems: [],
        sortOrderHigh: false,
        lastSortedKey: '',
        defaultSortOrder: []
    }
  }

  componentWillReceiveProps(nextProps){
    const {defaultSortOrder} = this.state;

    let defaultOrder = defaultSortOrder.length === 0 ?
      nextProps.overviewItems : defaultSortOrder;

    if ('overviewItems' in nextProps){
      this.setState({
        sortedOverviewItems: deepCopy(nextProps.overviewItems),
        defaultSortOrder: defaultOrder
      })
    }
  }

  render(){
    const {sortedOverviewItems} = this.state;

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
  							<th className="clickable" onClick={this.sort.bind(this, Sort.alphabetically, "folder")}>{"Folder"}</th>
  							<th className="clickable" onClick={this.sort.bind(this, Sort.byRank, "moneyReturned")}>{"Money returned"}</th>
  							<th className="clickable" onClick={this.sort.bind(this, Sort.byRank, "verifiedReturn")}>{"Return percentage"}</th>
  							<th className="clickable" onClick={this.sort.bind(this, Sort.byRank, "winPercentage")}>{"Win percentage"}</th>
  						</tr>
  					</thead>
  					<tbody>{tableItems}</tbody>
				</Table>);
  }

  sort = (func, param) => {
    const { state, props } = this;

    var sortOrderHigh = state.lastSortedKey === param ? !state.sortOrderHigh : true;

    if (sortOrderHigh
      && state.lastSortedKey === param
      && props.defaultSort !== undefined){

      this.setState({
        sortedOverviewItems: deepCopy(props.defaultSort),
        lastSortedKey: '',
        sortOrderHigh: false
      });

      return;
    }

    var sorted = func(this.state.sortedOverviewItems, param, sortOrderHigh);

    this.setState({
      sortedOverviewItems: sorted,
      sortOrderHigh: sortOrderHigh,
      lastSortedKey: param
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
