import React, { Component } from 'react';
import _ from 'lodash';
import { withRouter } from 'react-router-dom';
import * as pageActions from '../../actions/pageActions';
import * as pageUtils from '../../js/pageUtils'; 
import Header from '../../components/Header/Header';
import Menu from '../../components/Menu/Menu';
import Confirm from '../../components/Confirm/Confirm';
import Modal from '../../components/Page/Modal';
import PageContent from '../../components/Page/PageContent';
import Info from '../../components/Info/Info.jsx';
import './Page.css';

class Page extends Component
{
    constructor(props)
    {
		super(props);

		this.state = {
            actionModalOpen: false,
            actionModalProps: {},
            data: {},
            confirm: {
                show: false,
                action: '',
                actionDataKeys: [],
                header: ''
            }
        };
        
        this.clickModalAction = this.clickModalAction.bind(this);
        this.navigateTo = this.navigateTo.bind(this);
        this.clickPageAction = this.clickPageAction.bind(this);
        this.onDataChange = this.onDataChange.bind(this);
        this.executePageAction = this.executePageAction.bind(this);
        this.executePageActionFromConfirm = this.executePageActionFromConfirm.bind(this);
        this.handleDropdownServerUpdate = this.handleDropdownServerUpdate.bind(this);
    }

    componentDidMount()
    {
        const { components, data } = this.props;
        const stateData = _.cloneDeep(this.state.data);

        if (!_.isNil(components))
        {
            const initialData = pageUtils.getDataFromComponents(components, data);

            _.merge(stateData, initialData);

            this.setState({
                data: stateData
            });
        }
    }

    onDataChange(key, newValue)
    {
        this.setState({
            data: {
                ...this.state.data,
                [key]: newValue
            }
        });
    }

    /**
     * Handles a dropdown server update.
     * @param {string} dropdownKey 
     * @param {*} value 
     * @param {Array} componentsToUpdate 
     */
    handleDropdownServerUpdate(dropdownKey, value, componentsToUpdate)
    {
        const { props } = this;
        
        const components = _.map(componentsToUpdate, componentKey =>
            {
                return pageUtils.findComponentFromPage(
                    { components: props.components },
                    componentKey
                );
            })
        pageActions.handleServerDropdownUpdate(dropdownKey, value, components);
    }

    /**
     * Executes a page action after confirm.
     */
    executePageActionFromConfirm()
    {
        const { onSuccessNavigateTo } = this.state;
        const {action, actionDataKeys, componentsToInclude } = this.state.confirm;

        this.executePageAction(action, actionDataKeys, onSuccessNavigateTo, componentsToInclude)
    }

    /**
     * Executes a page action.
     * @param {string} action 
     * @param {Array} actionDataKeys 
     * @param {string} onSuccessNavigateTo Navigate to this address on success
     */
    executePageAction(action, actionDataKeys, onSuccessNavigateTo, componentsToInclude)
    {
        const { state, props } = this;

        const parameters = {};

        _.forEach(actionDataKeys, dataKey => 
        {
            if (!_.isNil(state.data[dataKey]))
            {
                parameters[dataKey] = state.data[dataKey];
            }
            // parameter in page data
            else 
            {
                parameters[dataKey] = props.data[dataKey];
            }
        });

        _.forEach(componentsToInclude, componentKey => 
        {
            parameters[componentKey] = pageUtils.findComponentFromPage(
                { components: props.components },
                componentKey);
        });

        const pathname = window.location.pathname;
        // Parse page name from path
        const pageKey = pathname.split('page/')[1];

        pageActions.callAction(pageKey, action, parameters, _.isNil(onSuccessNavigateTo)
            ? null
            : () => 
            {
                this.navigateTo(onSuccessNavigateTo);
            }
        );
    }

    /**
     * Handles page action button click. Either opens a confirm dialog
     * or calls action directly.
     * @param {string} action action name
     * @param {Array.<string>} actionDataKeys Data keys used in action.
     * @param {bool} requireConfirm Will a confirm dialog be displayed before completing action 
     * @param {string} confirmHeader Header for confirm dialog
     * @param {string} confirmStyle Confirm dialog style
     * @param {string} onSuccessNavigateTo If given, redirect to this address after successful action 
     */
    clickPageAction(
        action, 
        actionDataKeys, 
        requireConfirm, 
        componentsToInclude,
        confirmHeader, 
        confirmStyle, 
        onSuccessNavigateTo = null)
    {
        if (requireConfirm)
        {
            this.setState({
                confirm: {
                    show: true,
                    action,
                    actionDataKeys,
                    componentsToInclude,
                    header: confirmHeader,
                    variant: confirmStyle
                },
                onSuccessNavigateTo
            });
        }
        else 
        {
            this.executePageAction(action, actionDataKeys, onSuccessNavigateTo, componentsToInclude);
        }
    }

    /**
     * 
     * @param {string} action 
     * @param {Array.<object>} actionFields Fiels to be shown in action modal.
     * @param {title} title 
     * @param {bool} requireConfirm 
     * @param {string} confirmStyle 
     */
    clickModalAction(action, components, title, requireConfirm, confirmStyle)
    {
        this.setState({
            actionModalOpen: true,
            actionModalProps: {
                action,
                title,
                requireConfirm,
                confirmVariant: confirmStyle,
                components
            }
        });
    }

    navigateTo(address)
    {
        this.props.history.push(address);
    }

    /**
     * Returns function for button click event handling.
     * @param {object} button 
     */
    getButtonClick(button)
    {
        if (button.buttonType === 'ModalAction')
        {
            return this.clickModalAction;
        }

        if (button.buttonType === 'PageAction')
        {
            return this.clickPageAction;
        }

        if (button.buttonType === 'Navigation')
        {
            return this.navigateTo;
        }

        return null;
    }
    
    render()
    {
        const { state, props } = this;

        const pathname = window.location.pathname;
        // Parse page name from path and convert first char to upper
        let pageKey = pathname
            .substring(pathname.lastIndexOf('/') + 1);

        pageKey = pageKey
            .charAt(0)
            .toUpperCase() + pageKey.slice(1);

        return <div className="content">
            <Modal 
                onClose={() => 
                {
                    this.setState({ actionModalOpen: false });
                }} 
                page={pageKey}
                show={state.actionModalOpen} 
                {...state.actionModalProps}/>
            <Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disableValue={pageKey}></Menu>
            <Confirm 
                visible={state.confirm.show}
                headerText={state.confirm.header}
                cancelAction={() => 
                {
                    this.setState({
                        confirm: {
                            show: false,
                            action: '',
                            actionDataKeys: [],
                            header: ''
                        }
                    });
                }}
                confirmAction={this.executePageActionFromConfirm}
                variant={state.confirm.variant}/>
            <Info alertState={state.alertState} alertText={state.alertText} dismiss={this.dismissAlert} />
            <PageContent 
                components={props.components}
                className='page-content'
                getButtonClick={this.getButtonClick.bind(this)}
                onTableRowClick={this.navigateTo.bind(this)}
                onFieldValueChange={this.onDataChange.bind(this)}
                onHandleDropdownServerUpdate={this.handleDropdownServerUpdate}
                data={props.data}/>
        </div>;
    }
};

export default withRouter(Page);