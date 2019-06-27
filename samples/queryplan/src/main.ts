/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the Source EULA. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
'use strict';

import * as azdata from 'azdata';
import * as vscode from 'vscode';
const pd = require('pretty-data').pd;

let toggleOn: boolean = false;
let statusView = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right);
statusView.text = 'Query Plan watcher on';

async function onDidOpenTextDocument(doc: vscode.TextDocument): Promise<void> {	
	let queryDoc = await azdata.queryeditor.getQueryDocument(doc.uri.toString());
	if (queryDoc) {
		let options: Map<string, any> = new Map<string, any>();
		options['includeActualExecutionPlanXml'] = true;
		queryDoc.setExecutionOptions(options);
	}
}

let hasInitializedQueryOptions: boolean = false;

export function activate(context: vscode.ExtensionContext) {
	// set query options on open documents once the MSSQL connection provider is available
	setTimeout(function setOptionsWhenReady() {
		if (!hasInitializedQueryOptions) {		
			let connection = azdata.dataprotocol.getProvider<azdata.ConnectionProvider>('MSSQL', azdata.DataProviderType.ConnectionProvider);
			if (connection) {
				hasInitializedQueryOptions = true;
				let documents = vscode.workspace.textDocuments;
				documents.forEach((document) => {
					onDidOpenTextDocument(document);
				});
			} else {
				setTimeout(setOptionsWhenReady, 500);
			}
		}
	}, 500);

	vscode.commands.registerCommand('queryplan.ToggleProcessPlan', () => {
		toggleOn = !toggleOn;
		if (toggleOn) {
			statusView.show();
		} else {
			statusView.hide();
		}
	});

	vscode.workspace.onDidOpenTextDocument(params => onDidOpenTextDocument(params));

	azdata.queryeditor.registerQueryEventListener({
		onQueryEvent(type: azdata.queryeditor.QueryEvent, document: azdata.queryeditor.QueryDocument, args: any) {
			if (type === 'executionPlan') {
				if (toggleOn) {
					let tab2 = azdata.window.createTab('Web View Watcher');
					tab2.registerContent(async view => {
						let html: string = '<pre>\n';
						html += escapeAndLineBreak(<string>args);
						html += "\n</pre>";

						let webview1 = view.modelBuilder.webView()
							.withProperties<azdata.WebViewProperties>({
								html: html
							}).component();

						let divModel = view.modelBuilder.divContainer()
							.withItems([webview1], { CSSStyles: {  'height': '100%',  'width': '100%' } } )
							.withLayout({ width: '100%', height: '100%' })
							.component();

						await view.initializeModel(divModel);
					});

					document.createQueryTab(tab2);

					let tab = azdata.window.createTab('Form View Watcher');
					tab.registerContent(async view => {
						let planXml = <string>args;
						let fileNameTextBox = view.modelBuilder.inputBox().component();
						let xmlTextBox = view.modelBuilder.inputBox().component();

						let openButton = view.modelBuilder.button().withProperties({
							label: 'Open XML',
						}).component();

						openButton.onDidClick(async () => {
							try {
								planXml = pd.xml(planXml);
							} catch (e) {
								// If Xml fails to parse, fall back on original Xml content
							}

							vscode.workspace.openTextDocument({ language: 'xml' }).then((doc: vscode.TextDocument) => {
								vscode.window.showTextDocument(doc, 1, false).then(editor => {
									editor.edit(edit => {
										edit.insert(new vscode.Position(0, 0), planXml);
									}).then(result => {
										if (!result) {
											vscode.window.showErrorMessage('Cannot open file');
										}
									});
								}, (error: any) => {
									vscode.window.showErrorMessage(error);
								});
							}, (error: any) => {
								vscode.window.showErrorMessage(error);
							});
						});

						let formModel = view.modelBuilder.formContainer()
							.withFormItems([{
								component: fileNameTextBox,
								title: 'File name'
							}, {
								component: xmlTextBox,
								title: 'Plan XML'
							}, {
								component: openButton,
								title: 'Open XML'
							}]).withLayout({ width: '100%' }).component();							

						await view.initializeModel(formModel);

						fileNameTextBox.value = document.uri;
						xmlTextBox.value = planXml;
					});

					document.createQueryTab(tab);
				}
			}
		}
	});
}

// this method is called when your extension is deactivated
export function deactivate(): void {
}

export function escapeAndLineBreak(html: string): string {
	return html.replace(/[<|>|&|"]/g, function (match) {
		switch (match) {
			case '<': return '&lt;';
			case '>': return '&gt;\n';
			case '&': return '&amp;';
			case '"': return '&quot;';
			case '\'': return '&#39';
			default: return match;
		}
	});
}
